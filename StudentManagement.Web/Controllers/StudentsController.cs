using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagement.Web.Data;
using StudentManagement.Web.Models;

namespace StudentManagement.Web.Controllers;

public class StudentsController : Controller
{
    private readonly AppDbContext _context;

    public StudentsController(AppDbContext context)
    {
        _context = context;
    }

    // GET: Students
    public async Task<IActionResult> Index(string searchString)
    {
        var students = _context.Students.AsQueryable();

        if (!string.IsNullOrEmpty(searchString))
        {
            students = students.Where(s =>
                s.FirstName.Contains(searchString) || s.LastName.Contains(searchString)
            );
        }

        ViewBag.StudentCount = await students.CountAsync();
        return View(await students.ToListAsync());
    }

    // GET: Students/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
            return NotFound();

        var student = await _context.Students.FindAsync(id);
        if (student == null)
            return NotFound();

        return View(student);
    }

    // GET: Students/Create
    public IActionResult Create() => View();

    // POST: Students/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind("Id,FirstName,LastName,Email,Age")] Student student
    )
    {
        if (!ModelState.IsValid)
            return View(student);

        _context.Add(student);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // GET: Students/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
            return NotFound();

        var student = await _context.Students.FindAsync(id);
        if (student == null)
            return NotFound();

        return View(student);
    }

    // POST: Students/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        [Bind("Id,FirstName,LastName,Email,Age")] Student student
    )
    {
        if (id != student.Id)
            return NotFound();

        if (!ModelState.IsValid)
            return View(student);

        try
        {
            _context.Update(student);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!StudentExists(student.Id))
                return NotFound();
            throw;
        }

        return RedirectToAction(nameof(Index));
    }

    // GET: Students/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
            return NotFound();

        var student = await _context.Students.FindAsync(id);
        if (student == null)
            return NotFound();

        return View(student);
    }

    // POST: Students/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var student = await _context.Students.FindAsync(id);
        if (student != null)
            _context.Students.Remove(student);

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // GET: Students/Adults
    public async Task<IActionResult> Adults(string searchString)
    {
        var students = _context.Students.Where(s => s.Age > 18);

        if (!string.IsNullOrEmpty(searchString))
        {
            students = students.Where(s =>
                s.FirstName.Contains(searchString) || s.LastName.Contains(searchString)
            );
        }

        ViewBag.StudentCount = await students.CountAsync();
        return View(await students.ToListAsync());
    }

    private bool StudentExists(int id) => _context.Students.Any(e => e.Id == id);
}
