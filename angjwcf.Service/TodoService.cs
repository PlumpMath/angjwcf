using angjwcf.Service.Entity;
using angjwcf.Service.angjwcf.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel.Web;

namespace angjwcf.Service
{
    [ServiceContract]
    public interface ITodoService
    {
        [OperationContract]
        string Add(Todo todo);
        [OperationContract]
        void Delete(string id);
        [OperationContract]
        List<Todo> List();
        [OperationContract]
        bool Update(Todo todo);
        [OperationContract]
        Todo Get(string id);
    }

    public class TodoService:ITodoService
    {
        //List<Todo> _lstDb = new List<Todo>();
        //Nasty code for simplicity. Use dependency injection and separate the projects in real world app.
        ITodoPersistence _todoRepository;

        public TodoService()
        {
            _todoRepository = new TodoPersistence();
        }

        public string Add(Todo todo)
        {
            _todoRepository.Add(todo);
            return (todo.Id.ToString());
        }

        public Todo Get(string id)
        {
            return (_todoRepository.Get(Guid.Parse(id)));
        }

        public void Delete(string id)
        {
            _todoRepository.Delete(Guid.Parse(id));
        }

        //[WebGet(UriTemplate="/list", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        public List<Todo> List()
        {
            return (_todoRepository.List());
        }

        public bool Update(Todo todo)
        {
            return (_todoRepository.Update(todo));
        }
    }
}
