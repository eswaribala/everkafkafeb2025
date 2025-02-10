namespace PatientAPI.Repositories
{
    public interface IPatientPublishRepo
    {
        Task<string> PublishMessage(string Message,
            IConfiguration configuration);

    }
}
