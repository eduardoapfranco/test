namespace Application.AppServices.UserApplication.ViewModel
{
    public class UserControlAccessCategoriesViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }      
        public string Content { get; set; }
        public int Order { get; set; }   
        public string Icon { get; set; }
        public int? ParentId { get; set; }      
    }
}
