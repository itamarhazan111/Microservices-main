using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Play.Catalog.Service.Dtos;
using Play.Catalog.Service.Entities;
using Play.Common;


namespace Play.Catalog.Service.Controllers
{
    // https://localhost:[port]/api/items
    [ApiController]
    [Route("api/[controller]")]
    public class ItemsController : ControllerBase
    {
        private readonly IRepository<Item> itemsRepository;
        public ItemsController(IRepository<Item> itemsRepository){
            this.itemsRepository=itemsRepository;
        }

        [HttpGet]
        public async Task<IEnumerable<ItemDto>> Get()
        {
            var items = (await itemsRepository.GetAllAsync()).Select(item => item.AsDto());
            return items;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ItemDto>> GetById(Guid id)
        {
            var item = (await itemsRepository.GetAsync(id)).AsDto();
            if (item == null)
            {
                return NotFound();
            }
            return item;
        }

        [HttpPost]
        public async Task<ActionResult<ItemDto>> Post(CreateItemDto createItemDto)
        {
            var item = new Item { Name = createItemDto.Name, Description = createItemDto.Description, Price = createItemDto.Price, CreatedDate = DateTimeOffset.UtcNow };
            await itemsRepository.CreateAsync(item);
            return CreatedAtAction(nameof(GetById), new { id = item.Id }, item.AsDto());
        }

        // PUT https://localhost:[port]/api/items/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, UpdateItemDto updateItemDto)
        {
            var existingItem = await itemsRepository.GetAsync(id);
            if (existingItem == null)
            {
                return NotFound();
            }
            existingItem.Name = updateItemDto.Name;
            existingItem.Description = updateItemDto.Description;
            existingItem.Price = updateItemDto.Price;
            await itemsRepository.UpdateAsync(existingItem);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var item = await itemsRepository.GetAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            await itemsRepository.RemoveAsync(id);
            return NoContent();
        }
    }


}