using System.Collections.Generic;
namespace Fitness.Models;

public class UserToTrainerView
{
    public string Id { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
}

// Bu listeyi taşıyan ana ViewModel.
public class UserSelectionViewModel
{
    public List<UserToTrainerView> Users { get; set; }
}
