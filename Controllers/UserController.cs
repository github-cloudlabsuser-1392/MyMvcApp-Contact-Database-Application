using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MyMvcApp.Models;

namespace MyMvcApp.Controllers;

public class UserController : Controller
{
    public static System.Collections.Generic.List<User> userlist = new System.Collections.Generic.List<User>();

    // GET: User
    public ActionResult Index(string searchString)
    {
        var users = from u in userlist
                    select u;

        if (!String.IsNullOrEmpty(searchString))
        {
            users = users.Where(u => u.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase));
        }

        return View(users.ToList());
    }

    // GET: User/Details/5
    public ActionResult Details(int id)
    {
        var user = userlist.FirstOrDefault(u => u.Id == id);
        if (user == null)
        {
            return NotFound();
        }
        return View(user);
    }

    // GET: User/Create
    public ActionResult Create()
    {
        return View();
    }

    // POST: User/Create
    [HttpPost]
    public ActionResult Create(User user)
    {
        if (ModelState.IsValid)
        {
            userlist.Add(user);
            return RedirectToAction(nameof(Index));
        }
        return View(user);
    }

    // GET: User/Edit/5
    public ActionResult Edit(int id)
    {
        var user = userlist.FirstOrDefault(u => u.Id == id);
        if (user == null)
        {
            return NotFound();
        }
        return View(user);
    }

    // POST: User/Edit/5
    [HttpPost]
    public ActionResult Edit(int id, User user)
    {
        var existingUser = userlist.FirstOrDefault(u => u.Id == id);
        if (existingUser == null)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            existingUser.Name = user.Name;
            existingUser.Email = user.Email;
            // ...update other properties as needed...
            return RedirectToAction(nameof(Index));
        }
        return View(user);
    }

    // GET: User/Delete/5
    public ActionResult Delete(int id)
    {
        var user = userlist.FirstOrDefault(u => u.Id == id);
        if (user == null)
        {
            return NotFound();
        }
        return View(user);
    }

    // POST: User/Delete/5
    [HttpPost]
    public ActionResult Delete(int id, IFormCollection collection)
    {
        var user = userlist.FirstOrDefault(u => u.Id == id);
        if (user == null)
        {
            return NotFound();
        }

        userlist.Remove(user);
        return RedirectToAction(nameof(Index));
    }

    // GET: User/Search
    public ActionResult Search(string query)
    {
        if (string.IsNullOrEmpty(query))
        {
            return View(new List<User>());
        }

        var result = userlist.Where(u => u.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                                         u.Email.Contains(query, StringComparison.OrdinalIgnoreCase)).ToList();
        return View(result);
    }

    // GET: User/FindByName
    public ActionResult FindByName(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return View(new List<User>());
        }

        var result = userlist.Where(u => u.Name.Contains(name, StringComparison.OrdinalIgnoreCase)).ToList();
        return View(result);
    }
}
