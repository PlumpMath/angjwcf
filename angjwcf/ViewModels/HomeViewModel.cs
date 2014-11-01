using angjwcf.Common;
using angjwcf.Common.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace angjwcf.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {

        public ICommand GoTodoDetailsCmd { get; private set; }

        public ObservableCollection<TodoViewModel> TodoList { get; set; }
        private TodoViewModel _SelectedTodo;
        private int _TodoListSelectedIndex;
        private EditMode _TodoListEditMode;
        private IEventAggregator _eventAggregator;

        private string _RequestUrl;

        public string RequestUrl
        {
            get { return _RequestUrl; }
            set
            {
                if ((null != value) && (_RequestUrl != value))
                {
                    _RequestUrl = value;
                    OnPropertyChanged("RequestUrl");
                }
            }
        }

        public EditMode TodoListEditMode
        {
            get { return _TodoListEditMode; }
            set
            {
                if (_TodoListEditMode != value)
                {
                    _TodoListEditMode = value;
                    OnPropertyChanged("TodoListEditMode");
                }
            }
        }

        public int TodoListSelectedIndex
        {
            get { return _TodoListSelectedIndex; }
            set
            {
                if (_TodoListSelectedIndex != value)
                {
                    _TodoListSelectedIndex = value;
                    if (_TodoListSelectedIndex == -1)
                    {
                        TodoListEditMode = EditMode.Create;
                    }
                    else
                    {
                        TodoListEditMode = EditMode.Update;
                    }
                    OnPropertyChanged("TodoListSelectedIndex");
                }
            }
        }

        public TodoViewModel SelectedTodo
        {
            get { return _SelectedTodo; }
            set
            {
                if ((null != value) && (_SelectedTodo != value))
                {
                    _SelectedTodo = value;
                    OnPropertyChanged("SelectedTodo");
                }
            }
        }

        TodoServiceClient _todoServiceClient;


        public HomeViewModel()
        {
            //_todoServiceClient = BootStrapper.Instance.todoServiceClient;
            _eventAggregator = App.eventAggregator;
            TodoList = new ObservableCollection<TodoViewModel>();

            _TodoListSelectedIndex = -1;
            _SelectedTodo = new TodoViewModel();

            _RequestUrl = "http://localhost/Content/Html/Home.htm";

            GoTodoDetailsCmd = new RelayCommand(ExecGoTodoDetails, CanGoTodoDetails);
        }


        private bool CanAddTodo(object obj)
        {
            return (true);
        }

        private void ExecGoTodoDetails(object obj)
        {
            _eventAggregator.Publish<angjwcf.Views.NavMessage>(new Views.NavMessage(new Views.TodoDetails(SelectedTodo), null));
        }

        private bool CanGoTodoDetails(object obj)
        {
            return (true);
        }
    }
}
