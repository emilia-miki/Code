import redis

pool = redis.ConnectionPool(host='localhost', port=6379, db=0)
redis = redis.Redis(connection_pool=pool)


def tr(args):
    redis.set('count', 0)
    redis.incr('count')


redis.transaction(tr)
