using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRIS.Api.Migrations;

[DbContext(typeof(HRIS.Api.Data.AppDbContext))]
[Migration("20260930230000_BatchC_ApprovalSimple")]
public partial class BatchC_ApprovalSimple
{
    protected override void BuildTargetModel(ModelBuilder modelBuilder) { }
}
