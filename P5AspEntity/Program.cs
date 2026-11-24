using Microsoft.EntityFrameworkCore;
using P5AspEntity;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=etudiants.db"));

var app = builder.Build();

// Crée la base et la table automatiquement si elles n'existent pas encore
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

// GET : récupérer tous les étudiants
app.MapGet("/api/etudiants", async (AppDbContext db) =>
    await db.Etudiants.ToListAsync());

// GET : récupérer un étudiant par Id
app.MapGet("/api/etudiants/{id:int}", async (int id, AppDbContext db) =>
    await db.Etudiants.FindAsync(id) is Etudiant etudiant
        ? Results.Ok(etudiant)
        : Results.NotFound());

// POST : ajouter un étudiant
app.MapPost("/api/etudiants", async (Etudiant etudiant, AppDbContext db) =>
{
    if (string.IsNullOrWhiteSpace(etudiant.Nom) || etudiant.MtBourse <= 0)
    {
        return Results.BadRequest("Nom obligatoire et bourse strictement supérieure à 0.");
    }

    db.Etudiants.Add(etudiant);
    await db.SaveChangesAsync();
    return Results.Created($"/api/etudiants/{etudiant.Id}", etudiant);
});

// PUT : modifier un étudiant
app.MapPut("/api/etudiants/{id:int}", async (int id, Etudiant maj, AppDbContext db) =>
{
    var etudiant = await db.Etudiants.FindAsync(id);
    if (etudiant is null) return Results.NotFound();

    if (!string.IsNullOrWhiteSpace(maj.Nom))
        etudiant.Nom = maj.Nom;

    if (maj.MtBourse > 0)
        etudiant.MtBourse = maj.MtBourse;

    await db.SaveChangesAsync();
    return Results.Ok(etudiant);
});

// DELETE : supprimer un étudiant
app.MapDelete("/api/etudiants/{id:int}", async (int id, AppDbContext db) =>
{
    var etudiant = await db.Etudiants.FindAsync(id);
    if (etudiant is null) return Results.NotFound();

    db.Etudiants.Remove(etudiant);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.Run();