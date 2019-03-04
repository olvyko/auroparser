using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AuroParser
{
    public static class AuroMt940Parser
    {
        public static ICollection<CustomerStatementMessage> ParseData(string data, Mt940Parameters parameters)
        {
            if (string.IsNullOrWhiteSpace(data))
                throw new NullReferenceException("data cannot be null or empty");
            if (parameters == null)
                throw new NullReferenceException("parameters cannot be null");

            var customerStatementMessageDataList = CreateCustomerStatementMessageDataList(data, parameters);
            var customerStatementMessages = new List<CustomerStatementMessage>();
            foreach (var messageData in customerStatementMessageDataList)
            {
                var message = new CustomerStatementMessage(messageData, parameters);
                customerStatementMessages.Add(message);
            }
            return customerStatementMessages.Count != 0 ? customerStatementMessages : null;
        }

        private static ICollection<string> CreateCustomerStatementMessageDataList(string data, Mt940Parameters parameters)
        {
            if (!parameters.IsHeaderTrailerExists)
                return new List<string> { data };

            var headers = Regex.Matches(data, $"{parameters.Header}");
            var trailers = Regex.Matches(data, $"{parameters.Trailer}");
            if (headers.Count != trailers.Count)
                throw new Exception("headers count not equal to trailers count");

            List<string> messages = new List<string>();
            for (int i = 0; i < headers.Count; i++)
            {
                int startPos = headers[i].Index + parameters.Header.Length;
                int endPos = trailers[i].Index - startPos;
                var messageData = data.Substring(startPos, endPos);
                messages.Add(messageData.TrimEnd('\r', '\n').TrimStart('\r', '\n'));
            }
            return messages.Count != 0 ? messages : null;
        }
    }
}
