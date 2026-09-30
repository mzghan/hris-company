namespace HRIS.Api.Repositories;

// Menjalankan beberapa langkah Service (yang masing-masing memanggil SaveChanges) sebagai
// satu transaction. Aman dipanggil bersarang: kalau sudah ada transaction, langkah ikut di dalamnya.
public interface ITransactionRunner
{
    Task<T> RunAsync<T>(Func<Task<T>> action);
    Task RunAsync(Func<Task> action);
}
