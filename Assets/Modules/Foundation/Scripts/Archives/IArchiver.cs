
namespace Foundation.Archives {

    public interface IArchiver {
        void archive(ArchiveWriter archive);
        void unarchive(ArchiveReader archive);
    }

}