from django.db import models


class WorkPosition(models.IntegerChoices):
    BackendDeveloper = 0
    FrontendDeveloper = 1
    HR = 2
    TechSupport = 3
    QA = 4
    Marketer = 5
    Accountant = 6
    ProjectManager = 7


class EmployeeData(models.Model):
    phone = models.CharField(max_length=20)
    email = models.CharField(max_length=128)
    position = models.IntegerField(choices=WorkPosition.choices)
    bank_account = models.CharField(max_length=128)
    salary = models.DecimalField(max_digits=8, decimal_places=2)


class Person(models.Model):
    name = models.CharField(max_length=256)
    phone = models.CharField(max_length=20)
    email = models.CharField(max_length=128)
    subscription = models.BooleanField(null=True)
    employee_data = models.ForeignKey(EmployeeData, null=True, on_delete=models.SET_NULL)
