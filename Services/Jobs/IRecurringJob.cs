namespace HRIS.Api.Services.Jobs;

// Pekerjaan berkala di background. Batch berikutnya cukup menambah class baru dan mendaftarkannya
// di Program.cs (mis. reminder kelipatan 5 tahun, due date probation, dokumen ekspat kedaluwarsa).
public interface IRecurringJob
{
    string Name { get; }

    // Jarak antar eksekusi (dihitung dari selesainya eksekusi sebelumnya).
    TimeSpan Interval { get; }

    // services = scope baru per eksekusi, jadi aman meminta service/repository yang scoped.
    Task RunAsync(IServiceProvider services, CancellationToken cancellationToken);
}
