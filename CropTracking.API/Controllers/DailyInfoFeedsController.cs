﻿using CropTracking.API.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace CropTracking.API.Controllers
{

    [Route("api/[controller]")]

    public class DailyInfoFeedsController : Controller
    {
        [HttpGet()]
        public IActionResult GetDailyInformationFeeds()
        {
            return Ok(CropDataStore.Current.DailyInformationFeeds
                .OrderBy(sorted => sorted.CompanyName)
                .ThenBy(sorted => sorted.ReportDate));
        }

        [HttpGet("{id:int}", Name = "GetById")]
        public IActionResult GetById(int id)
        {
            var record = CropDataStore.Current.DailyInformation.SingleOrDefault(whatever => whatever.DailyInformationId == id);

            if (record == null)
            {
                return NotFound();
            }

            return Ok(record);

        }

        [HttpPost()]
        public IActionResult Create([FromBody] Models.DailyInfoDto dailyInfo)
        {
            if (dailyInfo == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            var maxDailyInfoId = CropDataStore.Current.DailyInformation.Max(p => p.DailyInformationId);

            var dinfo = new Models.DailyInformation()
            {
                DailyInformationId = ++maxDailyInfoId,
                PackShipDate = dailyInfo.PackShipDate,
                ReportDate = dailyInfo.ReportDate,
                Conventional44ozInventory = dailyInfo.Conventional44ozInventory,
                Organic60ozShipped = dailyInfo.Organic60ozShipped,
                Organic60ozPrice = dailyInfo.Organic60ozPrice

            };

            CropDataStore.Current.DailyInformation.Add(dinfo);


            CropDataStore.Current.Save();
            // TODO 2 - Call Save here to rewrite the file with all of the current data.
            //      Save is a method on the Current property of the CropDataStore. See if 
            //      you can figure out how to call that. It does not return anything.

            return CreatedAtRoute("GetById", new { id = dinfo.DailyInformationId }, dinfo);

        }


        [HttpPut("{id:int}")]
        public IActionResult Update(int id, [FromBody] Models.DailyInfoDto dailyInfo)
        {
            if (dailyInfo == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var record = CropDataStore.Current.DailyInformation.SingleOrDefault(whatever => whatever.DailyInformationId == id);

            if (record == null)
            {
                return NotFound();
            }

            record.PackShipDate = dailyInfo.PackShipDate;
            record.DailyInformationId = dailyInfo.DailyInformationId;
            record.ReportDate = dailyInfo.ReportDate;
            record.Conventional44ozInventory = dailyInfo.Conventional44ozInventory;
            record.Organic60ozShipped = dailyInfo.Organic60ozShipped;
            record.Organic60ozPrice = dailyInfo.Organic60ozPrice;

            CropDataStore.Current.Save();

            return NoContent();
        }

        [HttpPatch("{id:int}")]

        public IActionResult PartialUpdate(int id,
            [FromBody] JsonPatchDocument<DailyInfoDto> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            var idiots = CropDataStore.Current.DailyInformation.SingleOrDefault(whatever => whatever.DailyInformationId == id);
            if (idiots == null)
            {
                return NotFound();
            }

            var dailyInfoToPatch =
                    new DailyInfoDto()
                    {
                        PackShipDate = idiots.PackShipDate,
                        DailyInformationId = idiots.DailyInformationId,
                        ReportDate = idiots.ReportDate,
                        Conventional44ozInventory = idiots.Conventional44ozInventory,
                        Organic60ozShipped = idiots.Organic60ozShipped,
                        Organic60ozPrice = idiots.Organic60ozPrice
                    };
            patchDoc.ApplyTo(dailyInfoToPatch, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            idiots.PackShipDate = dailyInfoToPatch.PackShipDate;
            idiots.DailyInformationId = dailyInfoToPatch.DailyInformationId;
            idiots.ReportDate = dailyInfoToPatch.ReportDate;
            idiots.Conventional44ozInventory = dailyInfoToPatch.Conventional44ozInventory;
            idiots.Organic60ozShipped = dailyInfoToPatch.Organic60ozShipped;
            idiots.Organic60ozPrice = dailyInfoToPatch.Organic60ozPrice;


            CropDataStore.Current.Save();

            return NoContent();

        }

        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            var record = CropDataStore.Current.DailyInformation.FirstOrDefault(whatever => whatever.DailyInformationId == id);

            if (record == null)
            {
                return NotFound();
            }

            CropDataStore.Current.DailyInformation.Remove(record);

            CropDataStore.Current.Save();

            return NoContent();


        }


    }

}
