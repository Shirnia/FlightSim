import os
import platform
import signal
import sys
from re import A
from statistics import mode

import numpy as np
from gym_unity.envs import UnityToGymWrapper
from mlagents_envs.environment import UnityEnvironment
from stable_baselines3 import PPO, TD3
from stable_baselines3.common.callbacks import BaseCallback, EvalCallback
from stable_baselines3.common.env_util import make_vec_env
from stable_baselines3.common.monitor import Monitor
from stable_baselines3.common.noise import NormalActionNoise
from stable_baselines3.common.results_plotter import load_results, ts2xy
from stable_baselines3.common.utils import set_random_seed
from stable_baselines3.common.vec_env import DummyVecEnv, SubprocVecEnv
from torch import log_

env_config = {
    "unity_build_name": "FlightSim",
    # "unity_build_name": "FlightSim-v0",
    "env_path_windows": "Builds/",
    "env_path_linux"  : "/root/Unity/",
    "log_dir"         : "/logs/"
}

drl_config = {
    "num_cpu": 8,  # Number of processes to use
    "policy_type": "MlpPolicy",
    "total_timesteps": 10_000_000,
}

def make_env(rank, seed=0):
    """
    Utility function for multiprocessed env.

    :param env_id: (str) the environment ID
    :param num_env: (int) the number of environments you wish to have in subprocesses
    :param seed: (int) the inital seed for RNG
    :param rank: (int) index of the subprocess
    """
    def _init():
        # This is a non-blocking call that only loads the environment.
        unity_env = UnityEnvironment(file_name=env_config["unity_build_name"], 
            seed=seed+rank, worker_id=rank, no_graphics=True)
        env = UnityToGymWrapper(unity_env, allow_multiple_obs=False)
        env = Monitor(env, env_config["log_dir"], allow_early_resets=True) # Logs will be saved in log_dir/monitor.csv  
        return env
    set_random_seed(seed)
    return _init

# Handler for Ctrl+C to save model (not saved as onnx file)
def sigint_handler(signal, frame):
    print ('KeyboardInterrupt is caught')
    model.save("model.bak")
    sys.exit(0)

def linux_training(model,env):
    os.makedirs(env_config["env_path_linux"]+env_config["log_dir"], exist_ok=True)

    signal.signal(signal.SIGINT, sigint_handler) #signal to save model if interupted
    eval_callback = EvalCallback(env, best_model_save_path=env_config["log_dir"],
                             log_path=env_config["log_dir"], eval_freq=1000,
                             deterministic=True, render=False)

    model.learn(total_timesteps=drl_config["total_timesteps"],callback=eval_callback) #magic happens - values from https://www.kaggle.com/kwabenantim/gfootball-stable-baselines3

    env.close() #close environment
    model.save("model{}".format(drl_config["total_timesteps"])) #save model

def windows_training(model,env):
    os.makedirs(env_config["env_path_linux"]+env_config["log_dir"], exist_ok=True)

if __name__ == '__main__':
    if platform.system() == "Linux":
        env = SubprocVecEnv([make_env(i) for i in range(drl_config["num_cpu"])]) #vectorised environment
        model = PPO(drl_config["policy_type"], env, verbose=1) #network model 
        linux_training(model, env)
    else:
        env = SubprocVecEnv([make_env(i) for i in range(1)]) #vectorised environment
        model = PPO(drl_config["policy_type"], env, verbose=1) #network model 
        windows_training(model,env)


    
    # model = PPO.load("model{}".format(timesteps))

    # env.close()

    # model = PPO.load("model{}".format(timesteps))

    # unity_env_test = UnityEnvironment(file_name=env_id, seed=1, side_channels=[], no_graphics=True)
    # env_test = UnityToGymWrapper(unity_env_test, allow_multiple_obs=False)
    # obs = env_test.reset()
    # for i in range(1000):
    #     action, _states = model.predict(obs, deterministic=True)
    #     obs, reward, done, info = env_test.step(action)
    #     env_test.render()
    #     if done:
    #         obs = env_test.reset()
    # env_test.close()
