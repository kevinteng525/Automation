require 'test_helper'

class AutomationJobsControllerTest < ActionController::TestCase
  setup do
    @automation_job = automation_jobs(:one)
  end

  test "should get index" do
    get :index
    assert_response :success
    assert_not_nil assigns(:automation_jobs)
  end

  test "should get new" do
    get :new
    assert_response :success
  end

  test "should create automation_job" do
    assert_difference('AutomationJob.count') do
      post :create, automation_job: { create_by: @automation_job.create_by, description: @automation_job.description, job_type: @automation_job.job_type, modify_by: @automation_job.modify_by, name: @automation_job.name, priority: @automation_job.priority, retry_times: @automation_job.retry_times, status: @automation_job.status, sut_environment_id: @automation_job.sut_environment_id, test_agent_environment_id: @automation_job.test_agent_environment_id, time_out: @automation_job.time_out }
    end

    assert_redirected_to automation_job_path(assigns(:automation_job))
  end

  test "should show automation_job" do
    get :show, id: @automation_job
    assert_response :success
  end

  test "should get edit" do
    get :edit, id: @automation_job
    assert_response :success
  end

  test "should update automation_job" do
    patch :update, id: @automation_job, automation_job: { create_by: @automation_job.create_by, description: @automation_job.description, job_type: @automation_job.job_type, modify_by: @automation_job.modify_by, name: @automation_job.name, priority: @automation_job.priority, retry_times: @automation_job.retry_times, status: @automation_job.status, sut_environment_id: @automation_job.sut_environment_id, test_agent_environment_id: @automation_job.test_agent_environment_id, time_out: @automation_job.time_out }
    assert_redirected_to automation_job_path(assigns(:automation_job))
  end

  test "should destroy automation_job" do
    assert_difference('AutomationJob.count', -1) do
      delete :destroy, id: @automation_job
    end

    assert_redirected_to automation_jobs_path
  end
end
