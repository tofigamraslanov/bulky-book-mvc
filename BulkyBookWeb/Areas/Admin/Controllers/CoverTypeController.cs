﻿using BulkyBook.DataAccess.Repositories.Abstract;
using BulkyBook.Entities;
using BulkyBook.Utilities;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = StaticDetails.RoleAdmin)]
    public class CoverTypeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CoverTypeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Upsert(int? id)
        {
            var coverType = new CoverType();

            if (id == null)
                return View(coverType);

            var parameter = new DynamicParameters();
            parameter.Add("@Id", id);
            coverType =
                _unitOfWork.StoredProcedureCall.OneRecord<CoverType>(StaticDetails.ProcedureCoverTypeGet, parameter);

            if (coverType == null)
                return NotFound();

            return View(coverType);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(CoverType coverType)
        {
            if (!ModelState.IsValid)
                return View(coverType);

            var parameter = new DynamicParameters();
            parameter.Add("@Name", coverType.Name);

            if (coverType.Id == 0)
                _unitOfWork.StoredProcedureCall.Execute(StaticDetails.ProcedureCoverTypeCreate, parameter);
            else
            {
                parameter.Add("@Id", coverType.Id);
                _unitOfWork.StoredProcedureCall.Execute(StaticDetails.ProcedureCoverTypeUpdate, parameter);
            }

            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            var coverTypes =
                _unitOfWork.StoredProcedureCall.List<CoverType>(StaticDetails.ProcedureCoverTypeGetAll);
            return Json(new { data = coverTypes });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var parameter = new DynamicParameters();
            parameter.Add("@Id", id);
            var coverType =
                _unitOfWork.StoredProcedureCall.OneRecord<CoverType>(StaticDetails.ProcedureCoverTypeGet, parameter);

            if (coverType == null)
                return Json(new { success = false, message = "Error while deleting" });

            _unitOfWork.StoredProcedureCall.Execute(StaticDetails.ProcedureCoverTypeDelete, parameter);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Delete successful" });
        }

        #endregion
    }
}