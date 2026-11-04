using System.ComponentModel.DataAnnotations;

namespace G2AspNet.Models;

public enum ClassroomType
{
    [Display(Name = "Fisica")]
    Physic,

    [Display(Name = "Telematica")]
    Online
}