﻿

namespace Intro_EntityFramework.Models;

public class TaskItem
{
    public int Id { get; set; }
    
    public string Title { get; set; } = string.Empty;
    
    public bool IsCompleted { get; set; }

    public DateTime DueDate { get; set; }
}