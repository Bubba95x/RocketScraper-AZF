import unittest
import requests
from requests.models import Response
from RequestsRetry import RequestsRetry
import mock

class TestRetryRequests(unittest.TestCase):
    fakeUrl = 'https://fakenews.com'
    
    @mock.patch('RequestsRetry.requests')
    def test_get_happypath(self, mock_requests):
        # Arrange        
        fakeResponse = Response()
        fakeHeaders = { 'Authorization': 'Bearer kgabf' }
        fakeResponse.status_code = 200
        mock_requests.get.return_value = fakeResponse

        # Act
        result = RequestsRetry.get_retry_requests(url='https://fakenews.com/get', headers=fakeHeaders)
        print(result.status_code)
        
        # Assert
        self.assertEqual(fakeResponse.status_code, result.status_code)
        self.assertTrue(mock_requests.get.assert_called_once)

    @mock.patch('RequestsRetry.requests')
    def test_get_bad_request(self, mock_requests):
        # Arrange        
        fakeResponse = Response()
        fakeHeaders = { 'Authorization': 'Bearer kgabf' }
        fakeResponse.status_code = 401
        mock_requests.get.return_value = fakeResponse

        # Act
        result = RequestsRetry.get_retry_requests(url='https://fakenews.com/get', headers=fakeHeaders)
        print(result.status_code)
        
        # Assert
        print(mock_requests.get.mock_calls)
        self.assertEqual(fakeResponse.status_code, result.status_code)
        self.assertEqual(len(mock_requests.get.mock_calls), RequestsRetry.default_retries)

if __name__ == '__main__':
    unittest.main()