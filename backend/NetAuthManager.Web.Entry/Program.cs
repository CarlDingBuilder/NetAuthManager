using Furion.DatabaseAccessor;
using Furion;
using Microsoft.AspNetCore.HttpLogging;
using NetAuthManager.Core.Entities;
using NetAuthManager.EntityFramework.Core;
using Microsoft.EntityFrameworkCore;

Serve.Run(RunOptions.Default.WithArgs(args).ConfigureBuilder(builder =>
{
    // ������־��¼����
    builder.Services.AddHttpLogging(logging =>
    {
        //logging.LoggingFields = HttpLoggingFields.All;
        logging.LoggingFields = HttpLoggingFields.RequestPath |
                                HttpLoggingFields.RequestQuery |
                                HttpLoggingFields.RequestMethod |
                                HttpLoggingFields.RequestProperties |
                                HttpLoggingFields.RequestBody |
                                HttpLoggingFields.ResponseBody;
    });
}));