using EKO.ConnectFour.Domain;

namespace EKO.ConnectFour.Api.Services.Contracts;
//DO NOT TOUCH THIS FILE!!

public interface ITokenFactory
{
    string CreateToken(User user, IList<string> roleNames);
}