class TaskResultsController < ApplicationController
  def index
    if(params['ids']!=nil)
      @render_compare_table = true
      @task1_id = params['ids'].split(',')[0]
      @task2_id = params['ids'].split(',')[1]
      @task1_name = AutomationTask.get_force_automation_task_by_id(@task1_id).name
      @task2_name = AutomationTask.get_force_automation_task_by_id(@task2_id).name
      @array_of_result_array = TestResult.get_force_test_result_comparation_of_two_task(@task1_id, @task2_id)
    else
      @render_compare_table = false
    end
    #render :partial=>"test_results/test"
  end
end
