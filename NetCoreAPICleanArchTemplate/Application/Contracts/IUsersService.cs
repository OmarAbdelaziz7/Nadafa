

using Domain.Entities;
using Domain.Results;

namespace Application.Contracts
{
    public interface IUsersService
    {
        public Response<List<User>> getAll();
        public Response<User> get();
        public Response<User> add();
        public Response<User> update();
        public Response<string> delete();
    }
}
