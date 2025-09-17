using BlogCore.AccesoDatos.Data.Repository.IRepository;
using BlogCore.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogCore.AccesoDatos.Data.Repository
{
    public class UsuarioRepository : Repository<ApplicationUser>, IUsuarioRepository
    {
        private readonly DbContext _db;
        public UsuarioRepository(DbContext db) : base(db)
        {
            _db = db;
        }

        public void BloquearUsuario(string idUsuario)
        {
            var usuarioDesdeDb = _db.Set<ApplicationUser>().FirstOrDefault(u => u.Id == idUsuario);
            usuarioDesdeDb.LockoutEnd = DateTime.Now.AddYears(100);
            _db.SaveChanges();

        }

        public void DesbloquearUsuario(string idUsuario)
        {
            var usuarioDesdeDb = _db.Set<ApplicationUser>().FirstOrDefault(u => u.Id == idUsuario);
            usuarioDesdeDb.LockoutEnd = DateTime.Now;
            _db.SaveChanges();
        }
    }
}
