from rest_framework.serializers import ModelSerializer
from .models import Person, EmployeeData


class PersonSerializer(ModelSerializer):
    class Meta:
        model = Person
        fields = "__all__"

# from cw.models import Order
#
#
# class OrderSerializer(ModelSerializer):
#     class Meta:
#         model = Order
#         fields = '__all__'
