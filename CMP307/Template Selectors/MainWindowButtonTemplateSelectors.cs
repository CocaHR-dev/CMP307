using System.Windows;
using System.Windows.Controls;
using CMP307.Data;
using CMP307.Models;
using Microsoft.EntityFrameworkCore;

namespace CMP307.TemplateSelectors;

public class AddEditButtonTemplateSelector : DataTemplateSelector
{
    private readonly ScottishGlenContext _context;

    public AddEditButtonTemplateSelector(ScottishGlenContext context)
    {
        _context = context;
    }

    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
        var element = container as FrameworkElement;
        if (item is Employee employee)
        {
            if (_context.Entry(employee).State == EntityState.Detached)
                return element.FindResource("AddButtonTemplate") as DataTemplate;
            else
                return element.FindResource("EditButtonTemplate") as DataTemplate;
        }
        else if (item is Hardware hardware)
        {
            if (_context.Entry(hardware).State == EntityState.Detached)
                return element.FindResource("AddButtonTemplate") as DataTemplate;
            else
                return element.FindResource("EditButtonTemplate") as DataTemplate;
        }
        else if (item is Department department)
        {
            if (_context.Entry(department).State == EntityState.Detached)
                return element.FindResource("AddButtonTemplate") as DataTemplate;
            else
                return element.FindResource("EditButtonTemplate") as DataTemplate;
        }
        return null;
    }
}

public class DeleteButtonTemplateSelector : DataTemplateSelector
{
    private readonly ScottishGlenContext _context;

    public DeleteButtonTemplateSelector(ScottishGlenContext context)
    {
        _context = context;
    }

    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
        var element = container as FrameworkElement;
        if (item is Employee employee)
        {
            if (_context.Entry(employee).State != EntityState.Detached)
                return element.FindResource("DeleteButtonTemplate") as DataTemplate;
        }
        else if (item is Hardware hardware)
        {
            if (_context.Entry(hardware).State != EntityState.Detached)
                return element.FindResource("DeleteButtonTemplate") as DataTemplate;
        }
        else if (item is Department department)
        {
            if (_context.Entry(department).State != EntityState.Detached)
                return element.FindResource("DeleteButtonTemplate") as DataTemplate;
        }
        return null;
    }
}
