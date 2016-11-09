module AutomationJobsHelper
  def job_type_list
    AutomationJob::AUTOMATION_JOB_TYPES.collect{|t|[t,AutomationJob::AUTOMATION_JOB_TYPES.index(t)]}
  end
  def job_status_list
    AutomationJob::AUTOMATION_JOB_STATUS.collect{|t|[t,AutomationJob::AUTOMATION_JOB_STATUS.index(t)]}
  end
  def job_priority_list
    AutomationJob::AUTOMATION_JOB_PRIORITIES.collect{|t|[t,AutomationJob::AUTOMATION_JOB_PRIORITIES.index(t)]}
  end
  
  def job_type_name(index)
    if(index)
      AutomationJob::AUTOMATION_JOB_TYPES[index]
    else
      ''
    end
  end
  
  def job_status_name(index)
    if(index)
      AutomationJob::AUTOMATION_JOB_STATUS[index]
    else
      ''
    end
  end
  
  def job_priority_name(index)
    if(index)
      AutomationJob::AUTOMATION_JOB_PRIORITIES[index]
    else
      ''
    end 
  end
  
  def format_job_description(description)
    s=''
    if(description)
      description.split('|').each do |step|
        s += '<p>' + step + '</p>'
      end
      s      
    else
      ''
    end
  end

end
