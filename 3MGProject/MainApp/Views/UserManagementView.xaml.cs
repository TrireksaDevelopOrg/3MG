using DataAccessLayer.Bussines;
using DataAccessLayer.DataModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MainApp.Views
{
    /// <summary>
    /// Interaction logic for UserManagementView.xaml
    /// </summary>
    public partial class UserManagementView : Window
    {
        public UserManagementView()
        {
            InitializeComponent();
            this.DataContext = new UserManagementViewModel();
        }
    }

    public class UserManagementViewModel:Authorization
    {
        private UserManagement context = new UserManagement();
        public UserManagementViewModel():base(typeof(UserManagementViewModel))
        {
            AddNewUserCommand = new CommandHandler { CanExecuteAction = x => true, ExecuteAction = AddNewUserAction };
            var datas = context.GetUsers();
            allroles = context.GetRoles();
            Source = new  ObservableCollection<user>(datas);
            SourceView = (CollectionView)CollectionViewSource.GetDefaultView(Source);
            SourceView.Refresh();

            SourceRole = new ObservableCollection<role>();
            SourceViewRole = (CollectionView)CollectionViewSource.GetDefaultView(SourceRole);
        }

        private void AddNewUserAction(object obj)
        {
            var form = new Views.Registration();
            form.ShowDialog();
            var vm = (Views.RegistrationViewModel)form.DataContext;
            if(vm.UserCreated!=null)
            {
                Source.Add(vm.UserCreated);
                UserSelected = vm.UserCreated;
            }
        }

        private List<role> allroles;

        public ObservableCollection<user> Source { get; }
        public CollectionView SourceView { get; }
        public ObservableCollection<role> SourceRole { get; }
        public CollectionView SourceViewRole { get; }
        private user _user;

        public user UserSelected
        {
            get { return _user; }
            set {
                SetProperty(ref _user ,value);
                if(value!=null)
                {
                    var roles =_user.Roles().Result;
                    SourceRole.Clear();
                    foreach(var item in allroles)
                    {
                        var exist = roles.Where(O => O.Id == item.Id).FirstOrDefault();
                        if (exist != null)
                        {
                            item.Selected = true;
                        }
                        else
                            item.Selected = false;
                        item.PropertyChanged += Item_PropertyChanged;
                        SourceRole.Add(item);
                    }
                    SourceViewRole.Refresh();
                }
            }
        }

        public CommandHandler AddNewUserCommand { get; }

        private void Item_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var item = (role)sender;
            if(item.Selected)
            {
                context.AddUserInRole(UserSelected.Id, item.Name);
            }else
            {
                context.RemoveUserInRole(UserSelected.Id, item.Name);
            }
        }
    }
}
