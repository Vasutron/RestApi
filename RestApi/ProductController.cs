using System;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace RestApi;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    [HttpGet]
    [Route("[action]")]
    public IActionResult List() {
        try {
            using NpgsqlConnection conn = new MyConnect().GetConnection();
            using NpgsqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM tb_product ORDER BY id DESC";

            using NpgsqlDataReader reader = cmd.ExecuteReader(); // อ่านข้อมูลจากฐานข้อมูล
            List<ProductModel> list = new List<ProductModel>();  // สร้าง List เพื่อเก็บข้อมูลที่อ่านได้

            while (reader.Read()) {
                list.Add(new ProductModel{
                    Id = Convert.ToInt32(reader["id"]),
                    Barcode = reader["barcode"].ToString(),
                    Name = reader["name"].ToString(),
                    Price = Convert.ToInt32(reader["price"])
                });
            }
            return Ok(list);
            
        } catch (Exception ex) {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
        }
    }

    [HttpPost]
    [Route("[action]")]
    public IActionResult Create(ProductModel productModel) {
        try {
            using NpgsqlConnection conn = new MyConnect().GetConnection();
            using NpgsqlCommand cmd = conn.CreateCommand();

            cmd.CommandText = "INSERT INTO tb_product (barcode, name, price) VALUES (@barcode, @name, @price)";
            cmd.Parameters.AddWithValue("barcode", productModel.Barcode!);
            cmd.Parameters.AddWithValue("name", productModel.Name!);
            cmd.Parameters.AddWithValue("price", productModel.Price);

            if (cmd.ExecuteNonQuery() > 0) {
                return Ok(new { message = "Create success : " + productModel.Barcode });
            }
            return StatusCode(StatusCodes.Status501NotImplemented);

        } catch (Exception ex) {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
        }
    }

    [HttpPut]
    [Route("[action]")]
    public IActionResult Edit(ProductModel productModel) {
        try {
            using NpgsqlConnection conn = new MyConnect().GetConnection();
            using NpgsqlCommand cmd = conn.CreateCommand();

            cmd.CommandText = "UPDATE tb_product SET barcode = @barcode, name = @name, price = @price WHERE id = @id";
            cmd.Parameters.AddWithValue("barcode", productModel.Barcode!); // คำอธิบายโค้ด : กำหนดค่าให้กับพารามิเตอร์ barcode
            cmd.Parameters.AddWithValue("name", productModel.Name!); // คำอธิบายโค้ด : กำหนดค่าให้กับพารามิเตอร์ name
            cmd.Parameters.AddWithValue("price", productModel.Price); // คำอธิบายโค้ด : กำหนดค่าให้กับพารามิเตอร์ price
            cmd.Parameters.AddWithValue("id", productModel.Id); // คำอธิบายโค้ด : กำหนดค่าให้กับพารามิเตอร์ id
            if (cmd.ExecuteNonQuery() != 0)
            {
                return Ok(new { message = "Edit success : " + productModel.Id });
            }
            return StatusCode(StatusCodes.Status501NotImplemented);

        } catch (Exception ex) {
            return StatusCode(
                StatusCodes.Status500InternalServerError, 
                new { message = ex.Message }
                );
        }
        // return Ok("Edit : "+ productModel.Id);
    }

    [HttpDelete]
    [Route("[action]/{id}")]
    public IActionResult Remove(int id) {
        try{
            using NpgsqlConnection conn = new MyConnect().GetConnection();
            using NpgsqlCommand cmd = conn.CreateCommand();

            cmd.CommandText = "DELETE FROM tb_product WHERE id = @id";
            cmd.Parameters.AddWithValue("id", id);
            
            if (cmd.ExecuteNonQuery() != 0) {
                return Ok(new { message = "Remove success : " + id });
            }
            return StatusCode(StatusCodes.Status501NotImplemented);

        } catch (Exception ex) {
            return StatusCode(
                StatusCodes.Status500InternalServerError, 
                new { message = ex.Message }
                );
        }
        
    }
}
