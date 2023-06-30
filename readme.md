
## Sample Kusto query that was used in grafana 

```kusto
ChargeBackTable
| project repository, workflow_job
| extend StartTime = todatetime(workflow_job['started_at']), EndTime = todatetime(workflow_job['completed_at'])
| extend TimeConsumed = datetime_diff('second', EndTime, StartTime)
| summarize TimeConsumed = sum(TimeConsumed) by tostring(repository['name'])

```


## Architechture example

![image](https://github.com/SolidifyDemo/az-func-demo/assets/1208114/241083f5-2d74-4ef6-8590-f98a80f2b767)
