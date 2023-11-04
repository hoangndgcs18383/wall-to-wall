public static class EndpointConstants
{
    private const string URL = "https://zeff-web-service.onrender.com/";
    private const string LOGIN = "users/login";
    private const string SIGHUP = "users/signup";
    private const string GET_USER = "users/{id}";
    private const string USERS = "users";

    public const string GET_ALL_USERS = URL + USERS;
    public const string GET_USER_BY_ID = URL + GET_USER;
    public const string POST_CREATE = URL + SIGHUP;
    public const string POST_LOGIN = URL + LOGIN;
}