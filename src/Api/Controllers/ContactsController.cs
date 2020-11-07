using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PublicContacts.App.Actions;
using PublicContacts.App.Models;

namespace PublicContacts.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ContactsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ContactsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{id}")]
        [ProducesDefaultResponseType(typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ContactViewModel> GetById(int id) => await _mediator.Send(new GetContactByIdQuery { Id = id });

        [HttpGet]
        [ProducesDefaultResponseType(typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IEnumerable<ContactViewModel>> Get([FromQuery]GetContactsQuery query) => await _mediator.Send(query);

        [HttpPost]
        [ProducesDefaultResponseType(typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ContactViewModel> Create(CreateContactCommand command) => await _mediator.Send(command);

        [HttpPut("{id}")]
        [ProducesDefaultResponseType(typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ContactViewModel> Update(int id, UpdateContactCommand command)
        {
            command.Id = id;
            return await _mediator.Send(command);
        }

        [HttpDelete("{id}")]
        [ProducesDefaultResponseType(typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Remove(int id)
        {
            await _mediator.Send(new RemoveContactCommand { Id = id });
            return NoContent();
        }

        // PhoneNumbers
        [HttpPost("{contactId}/phone-numbers")]
        [ProducesDefaultResponseType(typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<PhoneNumberViewModel> AddPhoneNumber(int contactId, CreatePhoneNumberCommand command)
        {
            command.ContactId = contactId;
            return await _mediator.Send(command);
        }

        [HttpDelete("{contactId}/phone-numbers/{id}")]
        [ProducesDefaultResponseType(typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RemovePhoneNumber(int contactId, int id)
        {
            await _mediator.Send(new RemovePhoneNumberCommand { ContactId = contactId, Id = id });
            return NoContent();
        }
    }
}