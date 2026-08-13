using AutoMapper;
using BusinessLogicLayer.DTO;
using DataAccessLayer.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLogicLayer.Mappers
{
    public class StockReductionMappingProfile : Profile
    {
        public StockReductionMappingProfile()
        {
            CreateMap<OrderItemResponse, StockReduction>();
        }
    }
}
