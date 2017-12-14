using entity.Brillo;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace entity.Controller.Project
{
     public class ProjectController : Base
    {
        public int Count { get; set; }

        public int PageSize { get { return _PageSize; } set { _PageSize = value; } }
        public int _PageSize = 100;


        public int PageCount
        {
            get
            {
            
                return (Count % PageSize) < 1 ? 1 : (Count % PageSize) > 0 ? (Count / PageSize) + 1: (Count / PageSize);
                
            }
        }


        public async void Load(int PageIndex)
        {
            var predicate = PredicateBuilder.True<project>();
            predicate = predicate.And(x => x.id_company == CurrentSession.Id_Company);
            predicate = predicate.And(x => x.is_active);

            if (Count == 0)
            {
                Count = db.projects.Where(predicate).Count();
            }
            await db.projects.Where(predicate).Include(x => x.project_tag_detail).OrderByDescending(x => x.name)
                    .Skip(PageIndex * PageSize).Take(PageSize).LoadAsync();


            await db.app_dimension.Where(a => a.id_company == CurrentSession.Id_Company).LoadAsync();
            await db.app_property.LoadAsync();
            await db.app_measurement.Where(a => a.id_company == CurrentSession.Id_Company).LoadAsync();
        }






        public bool SaveChanges_WithValidation()
        {
            NumberOfRecords = 0;

            List<project_task> list = db.project_task.Local.ToList();

            foreach (project_task project_task in list)
            {
                if (project_task.State == EntityState.Added)
                {
                    project_task.timestamp = DateTime.Now;
                    project_task.State = EntityState.Unchanged;
                    db.Entry(project_task).State = EntityState.Added;

                    NumberOfRecords += 1;
                }
                else if (project_task.State == EntityState.Modified)
                {
                    project_task.timestamp = DateTime.Now;
                    project_task.State = EntityState.Unchanged;
                    db.Entry(project_task).State = EntityState.Modified;

                    NumberOfRecords += 1;
                }
                else if (project_task.State == EntityState.Deleted)
                {
                    project_task.timestamp = DateTime.Now;
                    project_task.State = EntityState.Unchanged;
                    db.Entry(project_task).State = EntityState.Deleted;

                    NumberOfRecords += 1;
                }
            }
            foreach (var error in db.GetValidationErrors())
            {
                db.Entry(error.Entry.Entity).State = EntityState.Detached;
            }

            if (db.GetValidationErrors().Count() > 0)
            {
                return false;
            }
            else
            {
                db.SaveChanges();
                return true;
            }
        }
    }
}
