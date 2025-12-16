using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Webapi.DAL;
using Webapi.Model;

namespace Webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoItemsWithSQLController : ControllerBase
    {
        [HttpGet]
        public ActionResult<TodoItem> GetTodoItem() {
            using var conn=Database.GetConnection();
            using var cmd=conn.CreateCommand();
            cmd.CommandText =
                 @"SELECT Id,Name,IsComplete,Secret FROM TodoItems";
            using var reader = cmd.ExecuteReader();
            if (!reader.Read())
                return NotFound();
            return new TodoItem
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                IsComplete = reader.GetBoolean(2),
               secret= reader.IsDBNull(3) ? null : reader.GetString(3)
            };
        }
        [HttpGet("{id}")]
        public ActionResult<TodoItem> GetTodoItem(int id)
        {
            using var conn = Database.GetConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText =
            @"SELECT Id, Name, IsComplete, Secret FROM TodoItems WHERE Id = $id";
            cmd.Parameters.AddWithValue("$id", id);
            using var reader = cmd.ExecuteReader();
            if (!reader.Read())
                return NotFound();
            return new TodoItem
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                IsComplete = reader.GetBoolean(2),
                secret = reader.IsDBNull(3) ? null : reader.GetString(3)
            };
        }
        [HttpPost]
        public ActionResult<TodoItem> PostTodoItem(TodoItem item)
        {
            using var conn = Database.GetConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText =
            @"INSERT INTO TodoItems (Name, IsComplete, Secret)
VALUES ($name, $isComplete, $secret);
SELECT last_insert_rowid();";
            cmd.Parameters.AddWithValue("$name", item.Name);
            cmd.Parameters.AddWithValue("$isComplete", item.IsComplete);
            cmd.Parameters.AddWithValue("$secret", item.secret);
            item.Id = Convert.ToInt32(cmd.ExecuteScalar());
            return CreatedAtAction(nameof(GetTodoItem), new { id = item.Id }, item);
        }
        [HttpPut("{id}")]
        public IActionResult PutTodoItem(int id, TodoItem item)
        {
            if (id != item.Id)
                return BadRequest();
            using var conn = Database.GetConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText =
            @"UPDATE TodoItems
SET Name=$name, IsComplete=$isComplete, Secret=$secret

WHERE Id=$id";
            cmd.Parameters.AddWithValue("$id", item.Id);
            cmd.Parameters.AddWithValue("$name", item.Name);
            cmd.Parameters.AddWithValue("$isComplete", item.IsComplete);
            cmd.Parameters.AddWithValue("$secret", item.secret);
            int rows = cmd.ExecuteNonQuery();
            if (rows == 0)
                return NotFound();
            return NoContent();
        }

    }
}
