namespace Api.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

using System;

public enum CargoType
{
    REGRIGERATED_GOODS = 0,
    GENERAL_CONSUMER_PRODUCTS = 1 ,
    ELECTRONICS = 2,
    HAZMAT = 3 ,
    OVERSIZED_INDUSTRIAL_EQUIPMENT = 4,
    OTHER = 5
}