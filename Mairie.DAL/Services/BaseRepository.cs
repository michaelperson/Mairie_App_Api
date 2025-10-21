using Mairie.DAL.Configuration;

namespace Mairie.DAL.Services
{
    public abstract class BaseRepository
    {
        protected DatabaseConfiguration _dbConfig;

        public BaseRepository(DatabaseConfiguration dbConfig)
        {
            _dbConfig = dbConfig ?? throw new ArgumentNullException(nameof(dbConfig));
        }
    }
}
