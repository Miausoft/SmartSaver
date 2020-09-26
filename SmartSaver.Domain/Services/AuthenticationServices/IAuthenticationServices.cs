using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SmartSaver.EntityFrameworkCore.Models;

namespace SmartSaver.Domain.Services.AuthenticationServices
{
    interface IAuthenticationServices
    {
        User Login(string username, string password);
        bool Register(string username, string password, string number);

    }
}
