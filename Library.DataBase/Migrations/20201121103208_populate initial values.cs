using System;
using Library.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Library.DataBase.Migrations
{
    public partial class populateinitialvalues : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var adminID = Guid.NewGuid();

            migrationBuilder.Sql($"Insert into Roles Values('{Guid.NewGuid()}', '{Global.ROLE_RESELLER}')");
            migrationBuilder.Sql($"Insert into Roles Values('{adminID}', '{Global.ROLE_ADMINISTRATOR}')");

            var adminUserID = Guid.NewGuid();
            var hasher = new PasswordHasher<SystemUser>();

            migrationBuilder.Sql($"Insert into Users Values('{adminUserID}', 'Library','Admin','admin@library.com','{hasher.HashPassword(new SystemUser(), "L1Brary_@dmin")}', 0, 1, 0,'admin@library.com')");
            migrationBuilder.Sql($"Insert into UserRoles Values('{adminUserID}', '{adminID}')");

            migrationBuilder.Sql($"Insert into Books Values('{Guid.NewGuid()}', 'Tale of Two Cities', 'Charles Dickens Classic A Tale of Two Cities is a novel set in London and Paris during the French Revolution. It''s focus is on the plight of the lower class of Paris subjugated by the elite with many paralleling London at the time.', 335)");
            migrationBuilder.Sql($"Insert into Books Values('{Guid.NewGuid()}', 'War and Peace', 'War and Peace is a novel by the Russian author Leo Tolstoy, first published serially, then published in its entirety in 1869. It is regarded as one of Tolstoy''s finest literary achievements and remains a classic of world literature.', 699)");
            migrationBuilder.Sql($"Insert into Books Values('{Guid.NewGuid()}', '1984', 'Winston Smith works for the Ministry of Truth in London, chief city of Airstrip One. Big Brother stares out from every poster, the Thought Police uncover every act of betrayal. When Winston finds love with Julia, he discovers that life does not have to be dull and deadening, and awakens to new possibilities. Despite the police helicopters that hover and circle overhead, Winston and Julia begin to question the Party; they are drawn towards conspiracy. Yet Big Brother will not tolerate dissent - even in the mind. For those with original thoughts they invented Room 101.', 165)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("Delete from UserRoles");
            migrationBuilder.Sql("Delete from Roles");
            migrationBuilder.Sql("Delete from Users");
            migrationBuilder.Sql("Delete from Books");
        }
    }
}
