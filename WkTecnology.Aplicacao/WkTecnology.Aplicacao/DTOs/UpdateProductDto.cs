﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portifolio.Aplicacao.DTOs
{
    public record UpdateProductDto(
        string Name,
        string Description,
        string Brand,
        string Model,
        int Year,
        string Color,
        int Mileage,
        int CategoryId 
        //bool IsActive
    );
}
