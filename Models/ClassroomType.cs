using System.ComponentModel.DataAnnotations;

namespace G2AspNet.Models;

public enum ClassroomType
{
    [Display(Name = "Presencial")]
    Physic,

    [Display(Name = "Telemática")]
    Online
}