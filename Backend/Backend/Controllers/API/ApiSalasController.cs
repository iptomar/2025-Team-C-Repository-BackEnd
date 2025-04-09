﻿using Backend.Data;
using Backend.DTO;
using Backend.Models;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiSalasController : ControllerBase {
        // ApplicationDbContext é a classe que representa a base de dados
        private readonly ApplicationDbContext _context;

        public ApiSalasController(ApplicationDbContext context) {
            _context = context;
        }
    }
}
