using MediatR;
using REPM.Application.CQRS.Commands;
using REPM.Application.DTOs;

namespace REPM.API.GraphQL.Mutations;

public class Mutation
{
    /// <summary>
    /// Default hello mutation to ensure the Mutation type is valid
    /// </summary>
    public string Hello() => "Hello from REPM GraphQL Mutations!";
}