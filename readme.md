
Sample Kusto query that was used in grafana 

```kusto
ChargeBackTable
| project repository, workflow_job
| extend StartTime = todatetime(workflow_job['started_at']), EndTime = todatetime(workflow_job['completed_at'])
| extend TimeConsumed = datetime_diff('second', EndTime, StartTime)
| summarize TimeConsumed = sum(TimeConsumed) by tostring(repository['name'])

```