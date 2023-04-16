from django.contrib import admin

# Register your models here.
from .models import EmployeeData, Person

admin.site.register(Person)
admin.site.register(EmployeeData)
