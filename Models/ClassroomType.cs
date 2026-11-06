using System.ComponentModel.DataAnnotations;

namespace G2AspNet.Models;

public enum ClassroomType
{
    [Display(Name = "Física")]
    Physic,

    [Display(Name = "Telemática")]
    Online
}