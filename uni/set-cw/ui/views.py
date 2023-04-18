from rest_framework.decorators import api_view
from rest_framework.response import Response
from rest_framework.request import Request
from .models import *
from orders.models import *
from orders import redis


@api_view(['POST', 'PUT'])
def orders(request: Request):
    if request.method == "POST":
        person = Person(**request.data["person"])
        person.save()
        request.data["order"]["customer_id"] = person.id
        to_submit = Order(**request.data["order"])
        db = redis.OrdersDB()
        result = db.add_order(to_submit)
        return Response({"id": result})
    elif request.method == "PUT":
        to_update = Order(**request.data["order"])
        db = redis.OrdersDB()
        result = db.change_order(to_update)
        return Response({"status": result})


@api_view(['GET'])
def order(request: Request, pk: str):
    db = redis.OrdersDB()
    result = db.get_order(pk)
    return Response({"order": result._asdict()})
