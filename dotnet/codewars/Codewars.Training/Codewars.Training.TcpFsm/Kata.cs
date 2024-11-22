namespace Codewars.Training.TcpFsm;

public static class Kata
{
    public static string TraverseStates(string[] events)
    {
        var state = "CLOSED";

        foreach (var e in events)
        {
            state = state switch
            {
                "CLOSED" when e == "APP_PASSIVE_OPEN" => "LISTEN",
                "CLOSED" when e == "APP_ACTIVE_OPEN" => "SYN_SENT",
                "LISTEN" when e == "RCV_SYN" => "SYN_RCVD",
                "LISTEN" when e == "APP_SEND" => "SYN_SENT",
                "LISTEN" when e == "APP_CLOSE" => "CLOSED",
                "SYN_RCVD" when e == "APP_CLOSE" => "FIN_WAIT_1",
                "SYN_RCVD" when e == "RCV_ACK" => "ESTABLISHED",
                "SYN_SENT" when e == "RCV_SYN" => "SYN_RCVD",
                "SYN_SENT" when e == "RCV_SYN_ACK" => "ESTABLISHED",
                "SYN_SENT" when e == "APP_CLOSE" => "CLOSED",
                "ESTABLISHED" when e == "APP_CLOSE" => "FIN_WAIT_1",
                "ESTABLISHED" when e == "RCV_FIN" => "CLOSE_WAIT",
                "FIN_WAIT_1" when e == "RCV_FIN" => "CLOSING",
                "FIN_WAIT_1" when e == "RCV_FIN_ACK" => "TIME_WAIT",
                "FIN_WAIT_1" when e == "RCV_ACK" => "FIN_WAIT_2",
                "CLOSING" when e == "RCV_ACK" => "TIME_WAIT",
                "FIN_WAIT_2" when e == "RCV_FIN" => "TIME_WAIT",
                "TIME_WAIT" when e == "APP_TIMEOUT" => "CLOSED",
                "CLOSE_WAIT" when e == "APP_CLOSE" => "LAST_ACK",
                "LAST_ACK" when e == "RCV_ACK" => "CLOSED",
                _ => "ERROR",
            };

            if (state == "ERROR") return state;
        }

        return state;
    }
}