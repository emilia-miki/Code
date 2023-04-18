import random
import signal
import time
import threading
import sys
import redis


def handler(signum, frame):
    global run_threads
    run_threads = False

    print("Terminating program...")
    t1.join()
    t2.join()
    f.close()

    try:
        sys.exit(0)
    except SystemExit:
        print("Program finished execution.")


def process1(running):
    i = 0
    while running():
        redis.lpush("queue", f"data{i}")
        i += 1
        time.sleep(random.random() * 2)


def process2(running, file):
    while running():
        data = redis.brpop(["queue"], 2)
        if data is None:
            break
        file.write(str(data[1])[2:-1] + "\n")


pool = redis.ConnectionPool(host='localhost', port=6379, db=0)
redis = redis.Redis(connection_pool=pool)
f = open("output.txt", "w")
run_threads = True

t1 = threading.Thread(target=process1, args=(lambda: run_threads,))
t2 = threading.Thread(target=process2, args=(lambda: run_threads, f,))

t1.start()
t2.start()

signal.signal(signal.SIGINT, handler)
