### Introduction to validation

Inputs of an application should be validated first. This input can be
sent by user or another application. In a web application, validation is
usually implemented twice: in client and in the server. Client-side
validation is implemented mostly for user experience. It's better to
check a form first in the client and show invalid fields to the user.
But, server-side validation is more critical and unavoidable.

Server side validation is generally implemented in [application
services](/Pages/Documents/Application-Services) or controllers (in
general, all services get data from presentation layer). An application
service method should first check (validate) input and then use it.
ASP.NET Boilerplate provides a good infrastructure to automatically
validate all inputs of an application for;

-   All [application service](Application-Services.html) methods
-   All [ASP.NET Core](AspNet-Core.html) MVC controller actions
-   All ASP.NET [MVC](MVC-Controllers.html) and [Web
    API](Web-API-Controllers.html) controller actions.

See Disabling Validation section to disable validation if needed.

### Using data annotations

ASP.NET Boilerplate supports data annotation attributes. Assume that
we're developing a Task application service that is used to create a
task and gets an input as shown below:

    public class CreateTaskInput
    {
        public int? AssignedPersonId { get; set; }

        [Required]
        public string Description { get; set; }
    }

Here, **Description** property is marked as **Required**.
AssignedPersonId is optional. There are also many attributes (like
MaxLength, MinLength, RegularExpression...) in
**System.ComponentModel.DataAnnotations** namespace. See Task
[application service](/Pages/Documents/Application-Services)
implementation:

    public class TaskAppService : ITaskAppService
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IPersonRepository _personRepository;

        public TaskAppService(ITaskRepository taskRepository, IPersonRepository personRepository)
        {
            _taskRepository = taskRepository;
            _personRepository = personRepository;
        }

        public void CreateTask(CreateTaskInput input)
        {
            var task = new Task { Description = input.Description };

            if (input.AssignedPersonId.HasValue)
            {
                task.AssignedPerson = _personRepository.Load(input.AssignedPersonId.Value);
            }

            _taskRepository.Insert(task);
        }
    }

As you see, no validation code is written since ASP.NET Boilerplate does
it automatically. ASP.NET Boilerplate also checks if input is **null**
and throws **AbpValidationException** if so. So, you don't have to write
**null-check** code (guard clause). It also throws
AbpValidationException if any of the input properties are invalid.

This machanism is similar to ASP.NET MVC's validation but notice that an
application service class is not derived from Controller, it's a plain
class and can work even out of a web application.

### Custom Validation

If data annotations are not sufficient for your case, you can implement
**ICustomValidate** interface as shown below:

    public class CreateTaskInput : ICustomValidate
    {
        public int? AssignedPersonId { get; set; }

        public bool SendEmailToAssignedPerson { get; set; }

        [Required]
        public string Description { get; set; }

        public void AddValidationErrors(CustomValidatationContext context)
        {
            if (SendEmailToAssignedPerson && (!AssignedPersonId.HasValue || AssignedPersonId.Value <= 0))
            {
                context.Results.Add(new ValidationResult("AssignedPersonId must be set if SendEmailToAssignedPerson is true!"));
            }
        }
    }

ICustomValidate interface declares **AddValidationErrors** method to be
implemented. We must add **ValidationResult** objects to
**context.Results** list if there are validation errors. You can also
use context.IocResolver to [resolve
dependencies](Dependency-Injection.html) if needed in validation
progress. 

In addition to ICustomValidate, ABP also supports .NET's standard
IValidatableObject interface. You can also implement it to perform
additional custom validations. If you implement both interfaces, both of
them will be called.

### Disabling Validation

For automatically validated classes (see Introduction section), you can
use these attributes to control validation:

-   **DisableValidation** attribute can be used for classes, methods or
    properties of DTOs to disable validation.
-   **EnableValidation** attribute can only be used to enable validation
    for a method, if it's disabled for the containing class.

### Normalization

We may need to perform an extra operation to arrange DTO parameters
after validation. ASP.NET Boilerplate defines **IShouldNormalize**
interface that has **Normalize** method for that. If you implement this
interface, Normalize method is called just after validation (and just
before method call). Assume that our DTO gets a Sorting direction. If
it's not supplied, we want to set a default sorting:

    public class GetTasksInput : IShouldNormalize
    {
        public string Sorting { get; set; }

        public void Normalize()
        {
            if (string.IsNullOrWhiteSpace(Sorting))
            {
                Sorting = "Name ASC";
            }
        }
    }
