from rest_framework.decorators import api_view
from rest_framework.response import Response
from rest_framework.request import Request
from . import redis
from .models import *


@api_view(["GET", "PUT"])
def orders(request: Request):
    db = redis.OrdersDB()
    if request.method == "GET":
        result = db.get_all_orders()
        return Response({"orders": [o._asdict() for o in result]})
    elif request.method == "PUT":
        return Response({"status": db.change_order(Order(**request.data["order"]))})


@api_view(["GET"])
def order(request: Request, pk: str):
    db = redis.OrdersDB()
    return Response({"order": db.get_order(pk)._asdict()})


@api_view(["GET"])
def get_customer(request: Request, pk: str):
    db = redis.OrdersDB()
    result = db.get_order(pk)
    if result is None:
        return Response({"customer_id": ""})
    else:
        return Response({"customer_id": result.customer_id})
