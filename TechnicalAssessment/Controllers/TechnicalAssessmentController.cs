using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Newtonsoft.Json;
using TechnicalAssessment.Models;

namespace TechnicalAssessment.Controllers
{
    public class TechnicalAssessmentController : ApiController
    {
        private List<Point> _existingPoints;
        private List<Point> _startPoints;
        private Point _startPoint;
        private bool _isFirstPerson;
        private int maxRowCount = 3;

        public TechnicalAssessmentController()
        {
            this._startPoint = new Point();
            this._startPoints = new List<Point>();
            this._existingPoints = new List<Point>();
            this._isFirstPerson = true;
            Point point = new Point() { xCoordinate = 1, yCoordinate = 1 };
            this._existingPoints.Add(point);
        }

        [Route("api/Initialize")]
        [HttpGet]
        public IHttpActionResult Initialize()
        {
            ResponseObject responseObject = new ResponseObject();
            responseObject.msg = "INITIALIZE";
            Body body = new Body();
            body.heading = "Player 1";
            body.message = "Awaiting Player 1's Move";
            responseObject.body = body;

            //var response = Request.CreateResponse(System.Net.HttpStatusCode.OK);
            //response.Content = new StringContent(responseObject.ToString(), System.Text.Encoding.UTF8, "application/json");
            return Ok(new {results = responseObject});
        }

        [Route("api/ValidateStartNode")]
        [HttpPost]
        public IHttpActionResult ValidateStartNode(RequestObject requestObject)
        {
            ResponseObject responseObject = new ResponseObject();
            Body body = new Body();

            if (_startPoints != null)
            {
                if (IsValidPoint(requestObject.point))
                {
                    if (_startPoints.Count == 0)
                    {
                        responseObject.msg = "VALID_START_NODE";
                        body.heading = "Player 1";
                        body.message = "Select a second node to complete the line.";
                        this._startPoint = requestObject.point;

                        this._isFirstPerson = true;
                    }
                    else
                    {
                        if (_startPoints.Contains(requestObject.point))
                        {
                            responseObject.msg = "VALID_START_NODE";
                            body.message = "Select a second node to complete the line.";
                            _startPoint = requestObject.point;
                            if (_isFirstPerson)
                            {
                                body.heading = "Player 1";
                            }
                            else
                            {
                                body.heading = "Player 2";

                            }
                        }
                        else
                        {
                            responseObject.msg = "INVALID_START_NODE";
                            body.message = "Not a valid starting position.";

                            if (_isFirstPerson)
                            {
                                body.heading = "Player 1";
                            }
                            else
                            {
                                body.heading = "Player 2";

                            }
                        }
                    }
                }
                else
                {
                    responseObject.msg = "INVALID_START_NODE";
                    body.message = "Not a valid starting position.";

                    if (_isFirstPerson)
                    {
                        body.heading = "Player 1";
                    }
                    else
                    {
                        body.heading = "Player 2";

                    }
                }
            }
            responseObject.body = body;

            return Ok(new { results = responseObject });
        }

        [Route("api/ValidateEndNode")]
        [HttpPost]
        public IHttpActionResult ValidateEndPoint(RequestObject requestObject)
        {
            ResponseObject responseObject = new ResponseObject();
            Body body = new Body();

            if (_existingPoints.Contains(requestObject.point) && !IsValidPoint(requestObject.point))
            {
                responseObject = GetInvalidResponseObject();
                return Ok(new { results = responseObject });
            }
            else
            {

                if (requestObject.point.xCoordinate == _startPoint.xCoordinate)
                {
                    if (requestObject.point.yCoordinate == _startPoint.xCoordinate)
                    {
                        if(requestObject.point.xCoordinate > _startPoint.xCoordinate)
                        {
                            int diffPoints = requestObject.point.xCoordinate - _startPoint.xCoordinate;
                            Point intermediatePoint = new Point();

                            for (int i = 0; i < diffPoints; i++)
                            {
                                intermediatePoint.xCoordinate = _startPoint.xCoordinate + 1;
                                intermediatePoint.yCoordinate = _startPoint.yCoordinate + 1;

                                if (_existingPoints.Contains(intermediatePoint) && !IsValidPoint(intermediatePoint))
                                {
                                    responseObject = GetInvalidResponseObject();
                                    return Ok(new { results = responseObject });
                                }
                                else
                                {
                                    _existingPoints.Add(intermediatePoint);
                                }
                            }

                            UpdateStartPoints(requestObject.point);
                            if(_isFirstPerson)
                            {
                                _isFirstPerson = false;
                            }
                            else
                            {
                                _isFirstPerson = true;
                            }
                            responseObject = GetResponseObject(requestObject.point);
                        }
                        else
                        {
                            Point intermediatePoint = new Point();

                            for (int i = requestObject.point.xCoordinate; i >= _startPoint.xCoordinate; i--)
                            {
                                intermediatePoint.xCoordinate = _startPoint.xCoordinate - 1;
                                intermediatePoint.yCoordinate = _startPoint.yCoordinate - 1;

                                if (_existingPoints.Contains(intermediatePoint) && !IsValidPoint(intermediatePoint))
                                {
                                    responseObject = GetInvalidResponseObject();
                                    return Ok(new { results = responseObject });
                                }
                                else
                                {
                                    _existingPoints.Add(intermediatePoint);
                                }
                            }

                            UpdateStartPoints(requestObject.point);
                            if (_isFirstPerson)
                            {
                                _isFirstPerson = false;
                            }
                            else
                            {
                                _isFirstPerson = true;
                            }
                            responseObject = GetResponseObject(requestObject.point);
                        }
                    }

                }
                else
                {
                    int diffX = Math.Abs(requestObject.point.xCoordinate - _startPoint.xCoordinate);
                    int diffY = Math.Abs(requestObject.point.yCoordinate - _startPoint.yCoordinate);

                    if(diffX == diffY)
                    {
                        Point intermediatePoint = new Point();
                        if(requestObject.point.xCoordinate > _startPoint.xCoordinate)
                        {
                            if(requestObject.point.yCoordinate > _startPoint.yCoordinate)
                            {
                                for (int i = 0; i < diffX;i++)
                                {
                                    intermediatePoint.xCoordinate = _startPoint.xCoordinate + 1;
                                    intermediatePoint.yCoordinate = _startPoint.yCoordinate + 1;
                                    if(IsValidPoint(intermediatePoint) && _existingPoints.Contains(intermediatePoint))
                                    {
                                        responseObject = GetInvalidResponseObject();
                                        return Ok(new { results = responseObject });
                                    }
                                    else
                                    {
                                        _existingPoints.Add(intermediatePoint);
                                    }
                                }

                                UpdateStartPoints(requestObject.point);
                                if (_isFirstPerson)
                                {
                                    _isFirstPerson = false;
                                }
                                else
                                {
                                    _isFirstPerson = true;
                                }
                                responseObject = GetResponseObject(requestObject.point);
                            }
                            else
                            {
                                for (int i = requestObject.point.yCoordinate; i >= _startPoint.yCoordinate; i--)
                                {
                                    intermediatePoint.xCoordinate = _startPoint.xCoordinate + 1;
                                    intermediatePoint.yCoordinate = _startPoint.yCoordinate - 1;

                                    if (_existingPoints.Contains(intermediatePoint) && !IsValidPoint(intermediatePoint))
                                    {
                                        responseObject = GetInvalidResponseObject();
                                        return Ok(new { results = responseObject });
                                    }
                                    else
                                    {
                                        _existingPoints.Add(intermediatePoint);
                                    }
                                }

                                UpdateStartPoints(requestObject.point);
                                if (_isFirstPerson)
                                {
                                    _isFirstPerson = false;
                                }
                                else
                                {
                                    _isFirstPerson = true;
                                }
                                responseObject = GetResponseObject(requestObject.point);
                            }
                        }
                        else
                        {
                            if (requestObject.point.yCoordinate > _startPoint.yCoordinate)
                            {
                                for (int i = 0; i < diffX; i++)
                                {
                                    intermediatePoint.xCoordinate = _startPoint.xCoordinate - 1;
                                    intermediatePoint.yCoordinate = _startPoint.yCoordinate + 1;
                                    if (IsValidPoint(intermediatePoint) && _existingPoints.Contains(intermediatePoint))
                                    {
                                        responseObject = GetInvalidResponseObject();
                                        return Ok(new { results = responseObject });
                                    }
                                    else
                                    {
                                        _existingPoints.Add(intermediatePoint);
                                    }
                                }

                                UpdateStartPoints(requestObject.point);
                                if (_isFirstPerson)
                                {
                                    _isFirstPerson = false;
                                }
                                else
                                {
                                    _isFirstPerson = true;
                                }
                                responseObject = GetResponseObject(requestObject.point);
                            }
                            else
                            {
                                for (int i = requestObject.point.yCoordinate; i >= _startPoint.yCoordinate; i--)
                                {
                                    intermediatePoint.xCoordinate = _startPoint.xCoordinate - 1;
                                    intermediatePoint.yCoordinate = _startPoint.yCoordinate - 1;

                                    if (_existingPoints.Contains(intermediatePoint) && !IsValidPoint(intermediatePoint))
                                    {
                                        responseObject = GetInvalidResponseObject();
                                        return Ok(new { results = responseObject });
                                    }
                                    else
                                    {
                                        _existingPoints.Add(intermediatePoint);
                                    }
                                }

                                UpdateStartPoints(requestObject.point);
                                if (_isFirstPerson)
                                {
                                    _isFirstPerson = false;
                                }
                                else
                                {
                                    _isFirstPerson = true;
                                }
                                responseObject = GetResponseObject(requestObject.point);
                            }
                        }
                    }
                    else
                    {
                        responseObject = GetInvalidResponseObject();
                        return Ok(new { results = responseObject });
                    }
                }
            }

            responseObject.body = body;
            return Ok(new { results = responseObject });
        }

        public ResponseObject GetInvalidResponseObject()
        {
            ResponseObject responseObject = new ResponseObject();
            Body body = new Body();
            responseObject.msg = "INVALID_END_NODE";
            body.message = "Invalid move!";

            if (_isFirstPerson)
            {
                body.heading = "Player 1";
            }
            else
            {
                body.heading = "Player 2";

            }

            return responseObject;
        }

        public ResponseObject GetResponseObject(Point endPoint)
        {
            ResponseObject responseObject = new ResponseObject();
            Body body = new Body();

            if (_startPoints != null && _startPoints.Count == 0)
            {
                responseObject.msg = "GAME_OVER";
                body.heading = "Game Over";

                NewLine newLine = new NewLine();
                newLine.start = _startPoint;
                newLine.end = endPoint;

                body.newLine = newLine;
                if (_isFirstPerson)
                {
                    body.message = "Player 1 Wins!";
                }
                else
                {
                    body.message = "Player 2 Wins!";
                }
            }
            else
            {
                responseObject.msg = "VALID_END_NODE";

                NewLine newLine = new NewLine();
                newLine.start = _startPoint;
                newLine.end = endPoint;
                body.newLine = newLine;
                if (_isFirstPerson)
                {
                    body.heading = "Player 1";
                }
                else
                {
                    body.heading = "Player 2";

                }
            }

            return responseObject;
        }

        public bool IsValidPoint(Point point)
        {
            bool isValidPoint = false;

            if((point.xCoordinate >= 0 && point.xCoordinate <= maxRowCount) &&
               (point.yCoordinate >= 0 && point.yCoordinate <= maxRowCount))
            {
                isValidPoint = true;
            }

            return isValidPoint;
        }

        public void UpdateStartPoints(Point endPoint)
        {
            List<Point> updatedStartingPoints = new List<Point>();
            foreach (var point in _startPoints)
            {
                if (IsValidStartPoint(point))
                {
                    updatedStartingPoints.Add(point);
                }
            }

            if(IsValidStartPoint(endPoint))
            {
                updatedStartingPoints.Add(endPoint);
            }

            this._startPoints = updatedStartingPoints;
        }

        public bool IsValidStartPoint(Point point)
        {
            Point sidePoint0 = new Point() { xCoordinate = point.xCoordinate + 1, yCoordinate = point.yCoordinate };
            Point sidePoint45 = new Point() { xCoordinate = point.xCoordinate + 1, yCoordinate = point.yCoordinate + 1 };
            Point sidePoint90 = new Point() { xCoordinate = point.xCoordinate, yCoordinate = point.yCoordinate + 1 };
            Point sidePoint135 = new Point() { xCoordinate = point.xCoordinate - 1, yCoordinate = point.yCoordinate + 1 };
            Point sidePoint180 = new Point() { xCoordinate = point.xCoordinate - 1, yCoordinate = point.yCoordinate };
            Point sidePoint225 = new Point() { xCoordinate = point.xCoordinate - 1, yCoordinate = point.yCoordinate - 1 };
            Point sidePoint270 = new Point() { xCoordinate = point.xCoordinate, yCoordinate = point.yCoordinate - 1 };
            Point sidePoint315 = new Point() { xCoordinate = point.xCoordinate + 1, yCoordinate = point.yCoordinate - 1 };
            bool isValidStartPoint = false;

            if (IsValidPoint(sidePoint0))
            {
                if (!_existingPoints.Contains(sidePoint0))
                {
                    isValidStartPoint = true;
                }
            }
            else if (IsValidPoint(sidePoint45))
            {
                if (!_existingPoints.Contains(sidePoint45))
                {
                    isValidStartPoint = true;
                }
            }
            else if (IsValidPoint(sidePoint90))
            {
                if (!_existingPoints.Contains(sidePoint90))
                {
                    isValidStartPoint = true;
                }
            }
            else if (IsValidPoint(sidePoint135))
            {
                if (!_existingPoints.Contains(sidePoint135))
                {
                    isValidStartPoint = true;
                }
            }
            else if (IsValidPoint(sidePoint180))
            {
                if (!_existingPoints.Contains(sidePoint180))
                {
                    isValidStartPoint = true;
                }
            }
            else if (IsValidPoint(sidePoint225))
            {
                if (!_existingPoints.Contains(sidePoint225))
                {
                    isValidStartPoint = true;
                }
            }
            else if (IsValidPoint(sidePoint270))
            {
                if (!_existingPoints.Contains(sidePoint0))
                {
                    isValidStartPoint = true;
                }
            }
            else if (IsValidPoint(sidePoint315))
            {
                if (!_existingPoints.Contains(sidePoint315))
                {
                    isValidStartPoint = true;
                }
            }

            return isValidStartPoint;
        }
    }
}